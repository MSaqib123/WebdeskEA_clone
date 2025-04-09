export class HttpClient {
    constructor(baseURL) {
        this.baseURL = baseURL;
        console.log("HttpClient initialized with base URL:", baseURL);
    }

    async get(action, params = {}) {
        try {
            const url = new URL(`${this.baseURL}/${action}`, window.location.origin);
            Object.keys(params).forEach(key => url.searchParams.append(key, params[key]));

            const response = await fetch(url, {
                method: 'GET',
                headers: { 'Content-Type': 'application/json' }
            });
            return await response.json();
        } catch (error) {
            console.error('GET request failed', error);
            throw error;
        }
    }

    async post(action, data) {
        try {
            const url = new URL(`${this.baseURL}/${action}`, window.location.origin);
            const response = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            return await response.json();
        } catch (error) {
            console.error('POST request failed', error);
            throw error;
        }
    }

    async put(action, data) {
        try {
            const url = new URL(`${this.baseURL}/${action}`, window.location.origin);
            console.log("PUT URL:", url); // Log the URL for debugging
            const response = await fetch(url, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });

            if (!response.ok) {
                const responseText = await response.text();
                console.error('PUT request failed with status:', response.status);
                console.error('Response text:', responseText);
                throw new Error('PUT request failed');
            }

            return await response.json();
        } catch (error) {
            console.error('PUT request failed', error);
            throw error;
        }
    }

    async delete(action, params = {}) {
        try {
            const url = new URL(`${this.baseURL}/${action}`, window.location.origin);
            Object.keys(params).forEach(key => url.searchParams.append(key, params[key]));

            const response = await fetch(url, {
                method: 'DELETE',
                headers: { 'Content-Type': 'application/json' }
            });
            return await response.json();
        } catch (error) {
            console.error('DELETE request failed', error);
            throw error;
        }
    }

    async getHtml(action, params = {}) {
        try {
            const url = new URL(`${this.baseURL}/${action}`, window.location.origin);
            Object.keys(params).forEach(key => url.searchParams.append(key, params[key]));

            const response = await fetch(url, {
                method: 'GET',
                headers: { 'Content-Type': 'text/html' }
            });
            return await response.text(); // Return HTML content as text
        } catch (error) {
            console.error('GET HTML request failed', error);
            throw error;
        }
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