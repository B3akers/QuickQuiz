import type { PageLoad } from './$types';
import { browser } from '$app/environment';
import { get } from '$lib/client/requests';
import { redirect } from '@sveltejs/kit';

export const prerender = true;

export const load: PageLoad = async ({ fetch }) => {
    if (browser) {

        let data = undefined;
        try {
            data = await get(fetch, '/moderator/question-reports');
        } catch {
            throw redirect(302, "/");
        }

        return { reportsData: data };
    }
};