<script lang="ts">
    import {
        Button,
        Tooltip,
        Input,
        Card,
        Table,
        TableBody,
        TableBodyCell,
        TableBodyRow,
        TableHead,
        TableHeadCell,
        Heading,
    } from "flowbite-svelte";
    import { UserName } from "$lib/components";
    import { getContext } from "svelte";
    import type { WebSocketManager } from "$lib/client/websocket";
    import { type Writable } from "svelte/store";

    import { UserSettingsModal, LobbySettingsModal, GameSettingsModal } from "$lib/components/modals";

    const session: any = getContext("session");
    const websocket: Writable<WebSocketManager> = getContext("websocket");
    const { lobby }: any = getContext("gameState");

    let lobbyOwner = $derived.by(() => {
        return $lobby.players.find((x: any) => x.id == $lobby.ownerId);
    });

    let isUserSettingsModalOpen = $state(false);
    let isLobbySettingsModalOpen = $state(false);
    let isGameSettingsModalOpen = $state(false);
</script>

<main
    class="flex-grow flex flex-col items-center justify-center space-y-4 mt-5 mb-5"
>
    <Card class="space-y-1">
        <span class="text-center">Lobby gracza</span>
        <span class="text-center">
            <UserName user={lobbyOwner} />
        </span>

        <hr />
        <div class="grid grid-cols-2 gap-2">
            <Input type="text" readonly value={$lobby.id} />
            <Button
                on:click={() =>
                    navigator.clipboard.writeText(
                        `${location.origin}/?lobby=${$lobby.id}`,
                    )}
                color="green"
                type="button">Zaproś do gry</Button
            >
            <Tooltip trigger="click">Skopiowano</Tooltip>
        </div>
        <hr />
        <span class="text-center"
            >Gracze: <b>{$lobby.players.length}</b>/<b>{$lobby.settings.maxPlayers}</b
            ></span
        >
        {#if lobbyOwner.id == $session.id}
            <Button
                on:click={() => {
                    $websocket?.sendMessage({
                        $type: "lobbyGameStart",
                    });
                }}
                color="green"
                disabled={$lobby.activeGameId != null}>Rozpocznij grę</Button
            >
        {/if}

        {#if $lobby.activeGameId}
            <Heading class="text-center" tag="h6">Gra jest w trakcie</Heading>
            <Button color="blue">Dołącz jako obserwator</Button>
        {/if}
    </Card>

    <Card class="space-y-4">
        <span class="text-center">Ustawienia</span>
        <hr />
        <div class="grid grid-cols-2 gap-2">
            <Button type="button" onclick={() => isGameSettingsModalOpen = true}>Ustawienia gry</Button>
            <Button type="button" onclick={() => isLobbySettingsModalOpen = true}>Ustawienia lobby</Button>
        </div>
        <hr />
        <span class="text-center">
            <UserName user={$session} />
        </span>
        <Button
            color="blue"
            type="button"
            onclick={() => (isUserSettingsModalOpen = true)}
            >Twoje ustawienia</Button
        >
        <Button
            on:click={() => {
                $websocket?.sendMessage({
                    $type: "lobbyPlayerQuit",
                });
            }}
            color="red"
            type="button">Opuść lobby</Button
        >
    </Card>

    <Card>
        <span class="text-center">List graczy</span>
        <div class="max-h-60 overflow-y-scroll">
            <Table hoverable={true}>
                <TableHead>
                    <TableHeadCell>Gracz</TableHeadCell>
                    <TableHeadCell>
                        <span class="sr-only">Akcja</span>
                    </TableHeadCell>
                </TableHead>
                <TableBody tableBodyClass="divide-y">
                    {#each $lobby.players as lobbyPlayer}
                        <TableBodyRow>
                            <TableBodyCell>
                                <UserName user={lobbyPlayer} /></TableBodyCell
                            >
                            <TableBodyCell>
                                {#if lobbyOwner.id == $session.id && lobbyPlayer.id != lobbyOwner.id}
                                    <div class="flex justify-between">
                                        <a
                                            href={null}
                                            onclick={() => {
                                                $websocket?.sendMessage({
                                                    $type: "lobbyPlayerKick",
                                                    playerId: lobbyPlayer.id,
                                                });
                                            }}
                                            class="font-medium text-red-500 hover:underline cursor-pointer"
                                            >Wyrzuć</a
                                        >
                                        <a
                                            href={null}
                                            onclick={() => {
                                                $websocket?.sendMessage({
                                                    $type: "lobbyPlayerPromote",
                                                    playerId: lobbyPlayer.id,
                                                });
                                            }}
                                            class="font-medium text-green-500 hover:underline cursor-pointer"
                                            >Promocja</a
                                        >
                                    </div>
                                {/if}
                            </TableBodyCell>
                        </TableBodyRow>
                    {/each}
                </TableBody>
            </Table>
        </div>
    </Card>
</main>

<UserSettingsModal bind:isOpen={isUserSettingsModalOpen} />
<LobbySettingsModal bind:isOpen={isLobbySettingsModalOpen}/>
<GameSettingsModal bind:isOpen={isGameSettingsModalOpen}/>