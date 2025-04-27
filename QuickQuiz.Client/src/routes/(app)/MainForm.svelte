<script lang="ts">
    import {
        Input,
        Label,
        Button,
        Card,
        Heading,
        Skeleton,
    } from "flowbite-svelte";
    import { UserName } from "$lib/components";
    import { goto } from "$app/navigation";
    import { superform } from "$lib/forms/superform";
    import { getContext, onMount } from "svelte";
    import { FormError, ErrorsAlert } from "$lib/components";
    import type { WebSocketManager } from "$lib/client/websocket";
    import { type Writable } from "svelte/store";
    import { page } from "$app/state";
    import { get } from "$lib/client/requests";
    import { UserSettingsModal } from "$lib/components/modals";

    const websocket: Writable<WebSocketManager> = getContext("websocket");

    const session: any = getContext("session");
    const superForm = superform({
        onUpdate({ data }) {
            if (data.ok) {
                $websocket?.sendMessage({
                    $type: "gameState",
                });
            }
        },
    });

    let statistics: any = $state(undefined);
    let isUserSettingsModalOpen = $state(false);

    async function loadStats() {
        statistics = await get(fetch, "/game/stats");
    }

    onMount(() => {
        loadStats();
    });

    const { enhance, errors, error, submitting } = superForm;
</script>

<main class="flex-grow flex flex-col items-center justify-center space-y-4">
    <Card>
        <span class="text-center"> <UserName user={$session} /></span></Card
    >
    <Card class="space-y-4">
        <form class="space-y-4" use:enhance>
            <ErrorsAlert error={$error} />
            <Label class="space-y-2">
                <span>Kod lobby</span>
                <Input
                    name="lobbyCode"
                    type="text"
                    value={page.url.searchParams.get("lobby") ?? ""}
                    disabled={$submitting}
                />
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
        <Button
            type="button"
            color="blue"
            onclick={() => (isUserSettingsModalOpen = true)}>Ustawienia</Button
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
    <Card>
        <Heading class="text-center" tag="h5">Statystyki</Heading>
        {#if statistics}
            <div class="flex justify-between">
                <div>
                    <span class="text-lg font-bold">Lobby</span>
                    <div class="flex justify-between space-x-5">
                        <span>Instancje:</span>
                        <span class="text-blue-600 font-semibold"
                            >{statistics.activeLobby}</span
                        >
                    </div>
                    <div class="flex justify-between space-x-5">
                        <span>Gracze:</span>
                        <span class="text-blue-600 font-semibold"
                            >{statistics.activeLobbyPlayers}</span
                        >
                    </div>
                </div>
                <div>
                    <span class="text-lg font-bold">Gra</span>
                    <div class="flex justify-between space-x-5">
                        <span>Instancje:</span>
                        <span class="text-blue-600 font-semibold"
                            >{statistics.activeGames}</span
                        >
                    </div>
                    <div class="flex justify-between space-x-5">
                        <span>Gracze:</span>
                        <span class="text-blue-600 font-semibold"
                            >{statistics.activePlayers}</span
                        >
                    </div>
                </div>
            </div>
        {:else}
            <Skeleton size="sm" />
        {/if}
    </Card>
</main>

<UserSettingsModal bind:isOpen={isUserSettingsModalOpen} />
