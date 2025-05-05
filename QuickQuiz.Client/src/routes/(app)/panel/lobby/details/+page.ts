import type { PageLoad } from './$types';
import { browser } from '$app/environment';
import { get } from '$lib/client/requests';
import { redirect } from '@sveltejs/kit';
import { HttpRequestError } from '$lib/client/requests';

export const prerender = true;

export const load: PageLoad = async ({ fetch, url }) => {
    if (browser) {
        let data = undefined;
        try {
            data = await get(fetch, `/moderator/lobby/${url.searchParams.get('id')}`);
        } catch (err) {
            if (err instanceof HttpRequestError) {
                if (err.status != 404) {
                    throw redirect(302, "/");
                }
            }
        }

        return data;
    }
};