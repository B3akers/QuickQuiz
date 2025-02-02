import type { LayoutLoad } from './$types';
import { browser } from '$app/environment';
import { get } from '$lib/client/requests';
import { redirect } from '@sveltejs/kit';

export const prerender = true;

export const load: LayoutLoad = async ({ fetch, url }) => {
    if (browser) {
        let session = undefined;

        try {
            session = await get(fetch, '/user/session');
        } catch { }

        if (session) {
            if (url.pathname === '/login') {
                throw redirect(302, "/");
            }
        } else {
            if (url.pathname !== '/login') {
                throw redirect(302, "/login");
            }
        }

        return { session: session };
    }
};