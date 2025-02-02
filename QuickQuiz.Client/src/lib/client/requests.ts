export const BACKEND_BASE_URL = 'https://localhost:7270';

class HttpRequestError extends Error {
    status: number;
    statusText: string;
    contentType: string;
    url: string;
    body: any;

    constructor(response: Response, body: any) {
        super(`HTTP error occured! status: ${response.status}, url: ${response.url}`);
        this.status = response.status;
        this.statusText = response.statusText;
        this.body = body;
        this.url = response.url;
        this.contentType = response.headers.get('content-type') ?? "";
    }
}

async function request(fetch: (input: RequestInfo | URL, init?: RequestInit) => Promise<Response>, url: string, options: any = {}) {
    if (!options.headers) options.headers = {};

    const session = window.localStorage.getItem('session');

    if (session) options.headers['Authorization'] = `Bearer ${session}`;

    const response = await fetch(`${BACKEND_BASE_URL}${url}`, options);

    const contentType = response.headers.get('content-type');

    if (!response.ok) {
        let errorBody = null;

        if (response.status === 401) {
            window.localStorage.removeItem('session');
        }

        if (contentType) {
            if (contentType.includes('json')) {
                errorBody = await response.json();
            } else if (contentType.includes('text/')) {
                errorBody = await response.text();
            }
        }

        const error = new HttpRequestError(response, errorBody);
        throw error;
    }

    if (contentType) {
        if (contentType.includes('json')) {
            return await response.json();
        } else if (contentType.includes('text/')) {
            return await response.text();
        } else if (contentType.includes('application/octet-stream')) {
            return await response.arrayBuffer();
        } else if (contentType.includes('multipart/form-data')) {
            return await response.formData();
        } else if (contentType.includes('image/')) {
            return await response.blob();
        }
    }

    return await response.text();
}

export function post(fetch: (input: RequestInfo | URL, init?: RequestInit) => Promise<Response>, url: string, body: any, additionalOptions : any = {}) {
    let headers = additionalOptions.headers || {};
    let processedBody = body;

    if (typeof body === 'object' && body !== null) {
        processedBody = JSON.stringify(body);
        headers['Content-Type'] = 'application/json';
    } else {
        headers['Content-Type'] = headers['Content-Type'] || 'text/plain';
    }

    return request(fetch, url, {
        method: 'POST',
        body: processedBody,
        headers: headers,
        ...additionalOptions
    });
}

export function get(fetch: (input: RequestInfo | URL, init?: RequestInit) => Promise<Response>, url: string, additionalOptions = {}) {
    return request(fetch, url, {
        method: 'GET',
        ...additionalOptions
    });
}