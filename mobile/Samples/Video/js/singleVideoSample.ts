
declare const XPMobileSDK: any;

const Application = new function () {
	/**
	 * Initialization. 
	 */
	const initialize = (): void => {
		const connectParams: object = { ProcessingMessage: 'No' };

		XPMobileSDK.Connect(connectParams, connectionDidConnect, connectionDidNotConnect);
	};

	/**
	 * Connection state observing. 
	 */
	const connectionDidNotConnect = (): void => {
		alert('Failed to Connect');
	};

	/**
	 * Connection state observing. 
	 */
	const connectionDidConnect = (): void => {
		if (sessionStorage.getItem('isLogIn') == String(true)) {
			const username: string = sessionStorage.getItem('Username');
			const password: string = sessionStorage.getItem('Password');

			login(username, password);
		}
		else {
			$('#login-modal').modal('show');
		}
	};

	/**
	 * Executes login process.
	 */
	const login = (username: string, password: string): void => {
		const loginParams: object = {
			'Username': username,
			'Password': password
		};

		sessionStorage.setItem('Username', username);
		sessionStorage.setItem('Password', password);

		XPMobileSDK.Login(loginParams, connectionDidLogIn, connectionDidNotLogIn);
	};

	/**
	 * Connection state observing. 
	 */
	const connectionDidNotLogIn = (): void => {
		document.querySelector('#alert').classList.remove('d-none');

		XPMobileSDK.connect(null);
	};

	/**
	 * Connection state observing. 
	 */
	const connectionDidLogIn = (): void => {
		$('#login-modal').modal('hide');

		sessionStorage.setItem('isLogIn', String(true));

		const searchParams: URLSearchParams = new URLSearchParams(location.search);
		const cameraId: string = searchParams.get('cid');

		if (cameraId === null) {
			window.alert('クエリーにカメラ ID を指定してください。');

			return;
		}

		XPMobileSDK.getAllViews(items => {
			const item: any = (items[0].Items[0].Items[0].Items as Array<any>).find(item => item.Id === cameraId.toLowerCase());

			if (item !== undefined) {
				buildCameraElement(item);
			}
		});
	};

	const RequestStreamParams = (cameraId: string, signalType: string): object => {
		return {
			CameraId: cameraId,
			DestWidth: 400,
			DestHeight: 300,
			SignalType: signalType /*'Live' or 'Playback'*/,
			MethodType: 'Push' /*'Pull'*/,
			Fps: 25, // This doesn't work for Pull mode, but we have to supply it anyway to keep the server happy
			ComprLevel: 71,
			KeyFramesOnly: 'No' /*'Yes'*/, // Server will give only key frame thumb nails. This will reduce FPS
			RequestSize: 'Yes',
			StreamType: 'Transcoded'
		};
	};

	const Camera = (cameraId: string): void => {
		const canvas: HTMLCanvasElement = document.querySelector('canvas');
		const canvasContext: CanvasRenderingContext2D = canvas.getContext('2d');
		const image: HTMLImageElement = document.createElement('img');
		let imageURL: string, videoController: any;
		let drawing: boolean = false;

		/**
		 * Video stream request callback 
		 */
		const requestStreamCallback = (videoConnection: any): void => {
			videoController = videoConnection;
			videoConnection.addObserver(videoConnectionObserver);
			videoConnection.open();
		};

		/**
		 * Executed on received frame. 
		 */
		const videoConnectionReceivedFrame = (frame: any): void => {
			if (drawing || frame.dataSize <= 0) {
				return;
			}

			drawing = true;

			if (frame.hasSizeInformation) {
				const multiplier: number = (frame.sizeInfo.destinationSize.resampling * XPMobileSDK.getResamplingFactor()) || 1;
				image.width = multiplier * frame.sizeInfo.destinationSize.width;
				image.height = multiplier * frame.sizeInfo.destinationSize.height;
			}

			if (imageURL) {
				window.URL.revokeObjectURL(imageURL);
			}

			imageURL = window.URL.createObjectURL(frame.blob);

			image.src = imageURL
		};

		/**
		 * Executed on image load. 
		 */
		const onImageLoad = (): void => {
			canvas.width = image.width;
			canvas.height = image.height;
			canvasContext.drawImage(image, 0, 0, canvas.width, canvas.height);

			drawing = false;
		};

		/**
		 * Executed on image load error. 
		 */
		const onImageError = (): void => {
			drawing = false;
		};

		const videoConnectionObserver = {
			videoConnectionReceivedFrame: videoConnectionReceivedFrame
		};

		image.addEventListener('load', onImageLoad);
		image.addEventListener('error', onImageError);

		XPMobileSDK.library.Connection.webSocketBrowser = false;

		/**
		 * Requesting a video stream. 
		 */
		XPMobileSDK.RequestStream(RequestStreamParams(cameraId, 'Live'), requestStreamCallback, () => { });
	};

	/**
	 * Builds camera element
	 */
	const buildCameraElement = (item: any): void => {
		document.querySelector('.card-header').innerHTML = item.Name;
		document.querySelector('.card').classList.remove('d-none');

		Camera(item.Id);
	};

	this.initialize = initialize;
	this.login = login;
};

document.addEventListener('DOMContentLoaded', () => {
	// Add the event listener when the login button is clicked.
	document.getElementById('login-button').addEventListener('click', () => {
		const username: string = (document.querySelector('#username') as HTMLInputElement).value;
		const password: string = (document.querySelector('#password') as HTMLInputElement).value;

		Application.login(username, password);
	});
});

window.addEventListener('load', () => {
	if (XPMobileSDK.isLoaded()) {
		Application.initialize();
	}
});
