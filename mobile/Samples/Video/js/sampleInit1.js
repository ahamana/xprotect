window.addEventListener('load', function () {

    console.log('Window loaded.');

    var startApp = function () {
        XPMobileSDK.onLoad = Application.initialize;
        XPMobileSDK.isLoaded() && Application.initialize();
    }
	
	if ('XPMobileSDK' in window) {
	    startApp();
	}
	else {
	    script = document.createElement('script');
	    script.addEventListener('load', function () {
	        startApp();
	    });
	    script.src = '../../XPMobileSDK.js';
	    document.querySelector('head').appendChild(script);
	}
});