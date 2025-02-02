<script lang="ts">
    import { Input, Label, Button, Card, Spinner } from "flowbite-svelte";
    import { TwitchSolid } from "$lib/icons";
    import { FormError } from "$lib/components";
    import { superform } from "$lib/forms/superform";
    import { goto } from "$app/navigation";

    let { data } = $props();

    const superForm = superform({
        onUpdate({ data }) {
            window.localStorage.setItem("session", data.token);
            goto("/", { invalidateAll: true });
        },
    });

    const { enhance, errors, submitting } = superForm;
</script>

<main class="flex-grow flex flex-col items-center justify-center">
    <Card class="space-y-4">
        <form method="POST" action="/user/create" class="space-y-4" use:enhance>
            <Label class="space-y-2">
                <span>Nazwa użytkownika</span>
                <Input name="username" type="text" disabled={$submitting} />
                {#if $errors.username}
                    <FormError errors={$errors.username} />
                {/if}
            </Label>
            <Button class="w-full" type="submit" disabled={$submitting}
                >Zaloguj się {#if $submitting}
                    <Spinner color="white" size="4" />
                {/if}
            </Button>
        </form>
        <hr />
        <Button
            href="https://id.twitch.tv/oauth2/authorize?response_type=code&client_id={data.twitchClientId}&redirect_uri={window
                .location.origin}/twitch-login&scope="
            type="button"
            class="w-full"
            disabled={$submitting}
            ><TwitchSolid color="purple" class="mr-2" />Zaloguj się za pomocą
            twitch</Button
        >
    </Card>
</main>
