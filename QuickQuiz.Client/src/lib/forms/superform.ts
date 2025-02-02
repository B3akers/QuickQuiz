import { BACKEND_BASE_URL } from "$lib/client/requests";
import { writable, type Writable } from "svelte/store";

type SuperForm = {
    enhance: Function;
    errors: Writable<any>;
    error: Writable<any>;
    submitting: Writable<boolean>;
};

type SuperFormOnUpdateParams = {
    form: SuperForm;
    data: any;
}

type SuperFormOptions = {
    onUpdate?: (params: SuperFormOnUpdateParams) => void;
};
export function superform(options: SuperFormOptions = {}) {
    async function handleSubmit(event: Event) {
        event.preventDefault();

        form.submitting.set(true);
        form.errors.set({});
        form.error.set(undefined);

        const submitEvent = event as SubmitEvent;
        const submitter = submitEvent.submitter;

        const formElement = event.target as HTMLFormElement;
        const formData = new FormData(formElement);
        const data = Object.fromEntries(formData.entries());

        const formAction = submitter?.getAttribute('formaction');
        const actionUrl = new URL(formElement.action);

        const requestUrl = formAction ? `${BACKEND_BASE_URL}${formAction}` : `${BACKEND_BASE_URL}${actionUrl.pathname}${actionUrl.search}${actionUrl.hash}`;

        const headers: any = {
            'Content-Type': 'application/json'
        };

        const session = window.localStorage.getItem('session');
        if (session) {
            headers['Authorization'] = `Bearer ${session}`;
        }

        try {
            const response = await fetch(requestUrl, {
                method: 'POST',
                headers: headers,
                body: JSON.stringify(data)
            });

            const contentType = response.headers.get('content-type');
            if (!contentType || !contentType.includes('json')) {
                throw new Error("Http request invalid response");
            }

            const jsonResponse = await response.json();
            if (!response.ok) {
                form.errors.update((errors) => {
                    if (jsonResponse.errors) {
                        for (const [key, value] of Object.entries(jsonResponse.errors)) {
                            errors[key] = value;
                        }
                    }
                    return errors;
                });
                if (jsonResponse.error) {
                    form.error.set(jsonResponse.error);
                }
                return;
            }

            if (options.onUpdate) options.onUpdate({ form: form, data: jsonResponse });
        } catch (e) {
            const error = e as Error;
            if (error) {
                form.error.set(error.message);
            }
        } finally {
            form.submitting.set(false);
        }
    }
    function enhanceForm(node: HTMLElement) {
        node.addEventListener('submit', handleSubmit);

        return {
            destroy() {
                node.removeEventListener('submit', handleSubmit, true);
            }
        };
    }
    let form: SuperForm = { enhance: enhanceForm, error: writable(), errors: writable({}), submitting: writable(false) };
    return form;
}