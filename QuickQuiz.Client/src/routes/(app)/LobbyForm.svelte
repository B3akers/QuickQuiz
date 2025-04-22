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

    const session: any = getContext("session");
    const websocket: Writable<WebSocketManager> = getContext("websocket");
    const game: Writable<any> = getContext("gameState");

    let lobbyOwner = $derived.by(() => {
        return $game.lobby.players.find(
            (x: any) => x.id == $game.lobby.ownerId,
        );
    });
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
            <Input type="text" readonly value={$game.lobby.id} />
            <Button
                on:click={() =>
                    navigator.clipboard.writeText(
                        `${location.origin}/?lobby=${$game.lobby.id}`,
                    )}
                color="green"
                type="button">Zaproś do gry</Button
            >
            <Tooltip trigger="click">Skopiowano</Tooltip>
        </div>
        <hr />
        <span class="text-center"
            >Gracze: <b>{$game.lobby.players.length}</b>/<b
                >{$game.lobby.maxPlayers}</b
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
                disabled={$game.lobby.activeGameId != null}
                >Rozpocznij grę</Button
            >
        {/if}

        {#if $game.lobby.activeGameId}
            <Heading class="text-center" tag="h6">Gra jest w trakcie</Heading>
            <Button color="blue">Dołącz jako obserwator</Button>
        {/if}
    </Card>

    <Card class="space-y-4">
        <span class="text-center">Ustawienia</span>
        <hr />
        <div class="grid grid-cols-2 gap-2">
            <Button type="button">Ustawienia gry</Button>
            <Button type="button">Ustawienia lobby</Button>
        </div>
        <hr />
        <span class="text-center">
            <UserName user={$session} />
        </span>
        <Button color="blue" type="button">Twoje ustawienia</Button>
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
                    {#each $game.lobby.players as lobbyPlayer}
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
