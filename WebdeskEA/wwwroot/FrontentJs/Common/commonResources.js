export class ReloadSources {
    constructor() {}

    async Reload() {
        await this.refreshResoursec([
            '~/Template/css/bootstrap.min.css',
            '~/Template/css/animate.css',
            '~/Template/plugins/select2/css/select2.min.css',
            '~/Template/plugins/owlcarousel/owl.carousel.min.css',
            '~/Template/css/dataTables.bootstrap4.min.css',
            '~/Template/plugins/fontawesome/css/fontawesome.min.css',
            '~/Template/plugins/fontawesome/css/all.min.css',
            '~/Template/css/style.css',
            '~/Template/js/jquery-3.6.0.min.js',
            '~/Template/js/feather.min.js',
            '~/Template/js/jquery.slimscroll.min.js',
            '~/Template/js/jquery.dataTables.min.js',
            '~/Template/js/dataTables.bootstrap4.min.js',
            '~/Template/js/bootstrap.bundle.min.js',
            '~/Template/plugins/select2/js/select2.min.js',
            '~/Template/plugins/owlcarousel/owl.carousel.min.js',
            '~/template/plugins/select2/js/custom-select.js',
            '~/Template/plugins/apexchart/apexcharts.min.js',
            '~/Template/plugins/apexchart/chart-data.js',
            '~/Template/js/script.js'
        ]);
    }

    async ReloadCommonFiles() {
        await this.refreshResoursec([
            '~/Template/css/style.css',
            '~/Template/js/script.js'
        ]);
    }

    async refreshResoursec(resources) {
        
            // Ensure that the base path meta tag exists
            var basePath = "/";
            var baseTag = document.querySelector('meta[name="base-url"]');
            if (baseTag) {
                basePath = baseTag.getAttribute('content');
            }

            // Remove all existing <script> and <link> tags in the header
            var head = document.head;

            // Remove all script tags
            var scriptTags = head.querySelectorAll('script');
            scriptTags.forEach(function (script) {
                head.removeChild(script);
            });

            // Remove all link tags
            var linkTags = head.querySelectorAll('link[rel="stylesheet"]');
            linkTags.forEach(function (link) {
                head.removeChild(link);
            });

            // Re-add each resource with a cache-busting query string
            resources.forEach(function (resourceUrl) {
                // Resolve the full URL
                var resolvedUrl = basePath + resourceUrl.replace('~/', '');

                if (resolvedUrl.endsWith('.css')) {
                    // Handle CSS
                    var newLink = document.createElement('link');
                    newLink.rel = 'stylesheet';
                    newLink.href = resolvedUrl + "?v=" + new Date().getTime();
                    newLink.onload = function () {
                        console.log("CSS refreshed: " + resolvedUrl);
                    };
                    newLink.onerror = function () {
                        console.error("Failed to refresh CSS: " + resolvedUrl);
                    };
                    head.appendChild(newLink);
                } else if (resolvedUrl.endsWith('.js')) {
                    // Handle JS
                    var newScript = document.createElement('script');
                    newScript.src = resolvedUrl + "?v=" + new Date().getTime();
                    newScript.onload = function () {
                        console.log("Script refreshed: " + resolvedUrl);
                    };
                    newScript.onerror = function () {
                        console.error("Failed to refresh script: " + resolvedUrl);
                    };
                    head.appendChild(newScript);
                }
            });
        
    }
}



//function createHttpClient(baseURL) {

//    console.log("HttpClient initialized with base URL:", baseURL);

//    async function get(action, params = {}) {
//        try {
//            const url = new URL(`${baseURL}/${action}`, window.location.origin);
//            Object.keys(params).forEach(key => url.searchParams.append(key, params[key]));

//            const response = await fetch(url, {
//                method: 'GET',
//                headers: { 'Content-Type': 'application/json' }
//            });
//            return await response.json();
//        } catch (error) {
//            console.error('GET request failed', error);
//            throw error;
//        }
//    }

//    async function post(action, data) {
//        try {
//            const url = new URL(`${baseURL}/${action}`, window.location.origin);
//            const response = await fetch(url, {
//                method: 'POST',
//                headers: { 'Content-Type': 'application/json' },
//                body: JSON.stringify(data)
//            });
//            return await response.json();
//        } catch (error) {
//            console.error('POST request failed', error);
//            throw error;
//        }
//    }

//    async function put(action, data) {
//        try {
//            const url = new URL(`${baseURL}/${action}`, window.location.origin);
//            const response = await fetch(url, {
//                method: 'PUT',
//                headers: { 'Content-Type': 'application/json' },
//                body: JSON.stringify(data)
//            });
//            return await response.json();
//        } catch (error) {
//            console.error('PUT request failed', error);
//            throw error;
//        }
//    }

//    async function del(action, params = {}) {
//        try {
//            const url = new URL(`${baseURL}/${action}`, window.location.origin);
//            Object.keys(params).forEach(key => url.searchParams.append(key, params[key]));

//            const response = await fetch(url, {
//                method: 'DELETE',
//                headers: { 'Content-Type': 'application/json' }
//            });
//            return await response.json();
//        } catch (error) {
//            console.error('DELETE request failed', error);
//            throw error;
//        }
//    }

//    async function getHtml(action, params = {}) {
//        try {
//            const url = new URL(`${baseURL}/${action}`, window.location.origin);
//            Object.keys(params).forEach(key => url.searchParams.append(key, params[key]));

//            const response = await fetch(url, {
//                method: 'GET',
//                headers: { 'Content-Type': 'text/html' }
//            });
//            //alert(await response.text())
//            return await response.text(); // Return HTML content as text
//        } catch (error) {
//            console.error('GET HTML request failed', error);
//            throw error;
//        }
//    }

//    return {
//        get,
//        post,
//        put,
//        del,
//        getHtml
//    };
//}