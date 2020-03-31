
const Application = new function () {
	this.initialize = initialize;
	this.login = login;

	/**
	 * Initialization. 
	 */
	function initialize() {
		const connectParams = { ProcessingMessage: 'No' };

		XPMobileSDK.Connect(connectParams, connectionDidConnect, connectionDidNotConnect);
	}

	/**
	 * Connection state observing. 
	 */
	function connectionDidNotConnect(parameters) {
		alert('Failed to Connect');
	}

	/**
	 * Connection state observing. 
	 */
	function connectionDidConnect(parameters) {
		if (sessionStorage.getItem('isLogIn') == String(true)) {
			const username = sessionStorage.getItem('Username');
			const password = sessionStorage.getItem('Password');

			login(username, password);
		}
		else {
			$('#login-modal').modal('show');
		}
	}

	/**
	 * Executes login process.
	 */
	function login(username, password) {
		const loginParams = {
			'Username': username,
			'Password': password
		};

		sessionStorage.setItem('Username', username);
		sessionStorage.setItem('Password', password);

		XPMobileSDK.Login(loginParams, connectionDidLogIn, connectionDidNotLogIn);
	}

	/**
	 * Connection state observing. 
	 */
	function connectionDidNotLogIn() {
		document.querySelector('#alert').classList.remove('d-none');

		XPMobileSDK.connect(null);
	}

	/**
	 * Connection state observing. 
	 */
	function connectionDidLogIn() {
		$('#login-modal').modal('hide');

		sessionStorage.setItem('isLogIn', String(true));

		const searchParams = new URLSearchParams(location.search);
		const cameraId = searchParams.get('cid');

		if (cameraId === null) {
			window.alert('クエリーにカメラ ID を指定してください。');

			return;
		}

		XPMobileSDK.getAllViews(items => {
			for (const item of items[0].Items[0].Items[0].Items) {
				if (item.Id !== cameraId.toLowerCase()) {
					continue;
				}

				buildCameraElement(item);
			}
		});
	}

	function RequestStreamParams(cameraId, signalType) {
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
	}

	function Camera(cameraId) {
		const canvas = document.querySelector('canvas');
		const canvasContext = canvas.getContext('2d');
		const image = document.createElement('img');
		image.addEventListener('load', onImageLoad);
		image.addEventListener('error', onImageError);
		let imageURL, videoController;
		let drawing = false;

		const videoConnectionObserver = {
			videoConnectionReceivedFrame: videoConnectionReceivedFrame
		}

		XPMobileSDK.library.Connection.webSocketBrowser = false;

		/**
		 * Requesting a video stream. 
		 */
		XPMobileSDK.RequestStream(RequestStreamParams(cameraId, 'Live'), requestStreamCallback, function (error) { });

		/**
		 * Video stream request callback 
		 */
		function requestStreamCallback(videoConnection) {
			videoController = videoConnection;
			videoConnection.addObserver(videoConnectionObserver);
			videoConnection.open();
		}

		/**
		 * Executed on received frame. 
		 */
		function videoConnectionReceivedFrame(frame) {
			if (drawing || frame.dataSize <= 0) {
				return;
			}

			drawing = true;

			if (frame.hasSizeInformation) {
				const multiplier = (frame.sizeInfo.destinationSize.resampling * XPMobileSDK.getResamplingFactor()) || 1;
				image.width = multiplier * frame.sizeInfo.destinationSize.width;
				image.height = multiplier * frame.sizeInfo.destinationSize.height;
			}

			if (imageURL) {
				window.URL.revokeObjectURL(imageURL);
			}

			imageURL = window.URL.createObjectURL(frame.blob);

			image.src = imageURL
		}

		/**
		 * Executed on image load. 
		 */
		function onImageLoad(event) {
			canvas.width = image.width;
			canvas.height = image.height;
			canvasContext.drawImage(image, 0, 0, canvas.width, canvas.height);

			drawing = false;
		}

		/**
		 * Executed on image load error. 
		 */
		function onImageError(event) {
			drawing = false;
		}
	};

	/**
	 * Builds camera element
	 */
	function buildCameraElement(item) {
		document.querySelector('.card-header').innerHTML = item.Name;
		document.querySelector('.card').classList.remove('d-none');

		Camera(item.Id);
	};
};

document.addEventListener('DOMContentLoaded', function () {
	// Add the event listener when the login button is clicked.
	document.getElementById('login-button').addEventListener('click', function () {
		const username = document.querySelector('#username').value;
		const password = document.querySelector('#password').value;

		Application.login(username, password);
	});
});

window.addEventListener('load', function () {
	if (XPMobileSDK.isLoaded()) {
		Application.initialize();
	}
});
