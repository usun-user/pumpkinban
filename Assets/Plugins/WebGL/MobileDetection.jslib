mergeInto(LibraryManager.library, {
    IsMobileDevice: function () {
        var userAgent = navigator.userAgent || navigator.vendor || window.opera;

        // iPhone, iPad, iPod, and Android devices
        if (/Android|iPhone|iPad|iPod/i.test(userAgent)) {
            return 1;
        }

        // Modern iPads can identify themselves as Mac computers
        if (navigator.platform === 'MacIntel' && navigator.maxTouchPoints > 1) {
            return 1;
        }

        return 0;
    }
});