<script lang="ts">
    import { Input, Label, Button, Card } from "flowbite-svelte";
    import { UserName } from "$lib/components";
    import { goto } from "$app/navigation";
    import { superform } from "$lib/forms/superform";
    import { getContext } from "svelte";
    import { FormError, ErrorsAlert } from "$lib/components";
    import type { WebSocketManager } from "$lib/client/websocket";
    import { type Writable } from "svelte/store";

    const websocket: Writable<WebSocketManager> = getContext("websocket");

    const session: any = getContext("session");
    const superForm = superform({
        onUpdate({ data }) {
            if (data.ok) {
                $websocket?.sendMessage(
                    JSON.stringify({
                        $type: "gameState",
                    }),
                );
            }
        },
    });

    const { enhance, errors, error, submitting } = superForm;
</script>

<main class="flex-grow flex flex-col items-center justify-center space-y-4">
    <Card>
        <UserName name={session.name} twitch={session.twitch} /></Card
    >
    <Card class="space-y-4">
        <form class="space-y-4" use:enhance>
            <ErrorsAlert error={$error} />
            <Label class="space-y-2">
                <span>Kod lobby</span>
                <Input name="lobbyCode" type="text" disabled={$submitting} />
                {#if $errors.lobbyCode}
                    <FormError errors={$errors.lobbyCode} />
                {/if}
            </Label>
            <div class="grid grid-cols-2 gap-2">
                <Button
                    formaction="/game/join-lobby"
                    disabled={$submitting}
                    type="submit">Dołącz do gry</Button
                >
                <Button
                    formaction="/game/create-lobby"
                    disabled={$submitting}
                    type="submit"
                    color="green">Stwórz lobby</Button
                >
            </div>
        </form>
        <hr />
        <Button href="/categories" type="button" color="blue">Przeglądaj</Button
        >
        <hr />
        <Button
            type="button"
            on:click={() => {
                window.localStorage.removeItem("session");
                goto("/login", { invalidateAll: true });
            }}
            color="red">Wyloguj się</Button
        >
    </Card>
</main>
