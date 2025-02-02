import type { LayoutLoad } from './$types';
import { browser } from '$app/environment';
import { post } from '$lib/client/requests';
import { redirect } from '@sveltejs/kit';

export const prerender = true;

export const load: LayoutLoad = async ({ fetch, url }) => {
    if (browser) {
        try {
            const result = await post(fetch, '/user/twitch-login', {
                code: url.searchParams.get('code'),
                redirectUrl: `${url.origin}${url.pathname}`
            });

            window.localStorage.setItem('session', result.token);
        } catch { }

        throw redirect(302, '/');
    }
};