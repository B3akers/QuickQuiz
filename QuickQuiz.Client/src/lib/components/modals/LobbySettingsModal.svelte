<script lang="ts">
    import { getContext } from "svelte";
    import {
        Modal,
        Toggle,
        Helper,
        Input,
        Label,
        Button,
    } from "flowbite-svelte";
    import type { WebSocketManager } from "$lib/client/websocket";
    import { type Writable } from "svelte/store";

    const session: any = getContext("session");
    const { lobby }: any = getContext("gameState");

    const websocket: Writable<WebSocketManager> = getContext("websocket");

    let localIsOwner = $derived.by(() => {
        return $lobby.ownerId == $session.id;
    });

    let twitchMode = $state($lobby.settings.twitchMode);
    let maxPlayers = $state($lobby.settings.maxPlayers);

    let { isOpen = $bindable() as boolean } = $props();
</script>

<Modal size="xs" title="Ustawienia lobby" bind:open={isOpen} outsideclose autoclose>
    <Toggle disabled={!localIsOwner} bind:checked={twitchMode}
        >Twitch mode</Toggle
    >
    <Helper class="select-none">
        Dołączenie do lobby wymaga zalogowania się przez twitch.
    </Helper>

    <Label class="font-bold">Maksymalna liczba osób</Label>
    <Input
        onblur={() => {
            if (maxPlayers < $lobby.players.length) {
                maxPlayers = $lobby.players.length;
            }

            if (maxPlayers > 1000) {
                maxPlayers = 1000;
            }
        }}
        disabled={!localIsOwner}
        type="number"
        min={$lobby.players.length}
        max={1000}
        bind:value={maxPlayers}
    />

    <Button
        disabled={!localIsOwner}
        onclick={() => {
            $websocket?.sendMessage({
                $type: "lobbyUpdateSettings",
                settings: {
                    twitchMode,
                    maxPlayers,
                },
            });
        }}>Zapisz</Button
    >
</Modal>