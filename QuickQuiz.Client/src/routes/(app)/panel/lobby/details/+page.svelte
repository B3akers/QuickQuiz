<script lang="ts">
    import {
        Card,
        Button,
        Heading,
        Table,
        TableHead,
        TableHeadCell,
        TableBody,
        TableBodyRow,
        TableBodyCell,
    } from "flowbite-svelte";
    import { UserName } from "$lib/components";
    import {
        LobbySettingsModal,
        GameSettingsModal,
    } from "$lib/components/modals";
    let { data }: any = $props();

    let lobbyOwner = $derived.by(() => {
        return data.lobby.players.find((x: any) => x.id == data.lobby.ownerId);
    });

    let isLobbySettingsModalOpen = $state(false);
    let isGameSettingsModalOpen = $state(false);
</script>

<main class="flex-grow flex flex-col items-center justify-center space-y-4 m-5">
    {#if data.lobby}
        <Card class="space-y-1">
            <Heading tag="h6" class="text-center">{data.lobby.id}</Heading>
            <span class="text-center">Lobby gracza</span>
            <span class="text-center">
                <UserName user={lobbyOwner} />
            </span>
            <hr />
            <span class="text-center"
                >Gracze: <b>{data.lobby.players.length}</b>/<b
                    >{data.lobby.settings.maxPlayers}</b
                ></span
            >
            <div class="flex justify-between gap-2">
                <Button
                    on:click={() => {}}
                    class="w-full"
                    color="green"
                    disabled={data.lobby.activeGameId != null}
                    >Rozpocznij grę</Button
                >
                <Button on:click={() => {}} class="w-full" color="red"
                    >Rozwiąż lobby</Button
                >
            </div>
            {#if data.lobby.activeGameId}
                <Heading class="text-center" tag="h6"
                    >Gra jest w trakcie</Heading
                >
            {/if}
            <span class="text-center">Ustawienia</span>
            <div class="grid grid-cols-2 gap-2">
                <Button
                    type="button"
                    onclick={() => (isGameSettingsModalOpen = true)}
                    >Ustawienia gry</Button
                >
                <Button
                    type="button"
                    onclick={() => (isLobbySettingsModalOpen = true)}
                    >Ustawienia lobby</Button
                >
            </div>
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
                        {#each data.lobby.players as lobbyPlayer}
                            <TableBodyRow>
                                <TableBodyCell>
                                    <UserName
                                        user={lobbyPlayer}
                                    /></TableBodyCell
                                >
                                <TableBodyCell>
                                    <div class="flex justify-between">
                                        <a
                                            href={null}
                                            onclick={() => {}}
                                            class="font-medium text-red-500 hover:underline cursor-pointer"
                                            >Wyrzuć</a
                                        >
                                        {#if lobbyPlayer.id != data.lobby.ownerId}
                                            <a
                                                href={null}
                                                onclick={() => {}}
                                                class="font-medium text-green-500 hover:underline cursor-pointer"
                                                >Promocja</a
                                            >
                                        {/if}
                                    </div>
                                </TableBodyCell>
                            </TableBodyRow>
                        {/each}
                    </TableBody>
                </Table>
            </div>
        </Card>

        <LobbySettingsModal
            bind:isOpen={isLobbySettingsModalOpen}
            canModify={true}
            settings={data.lobby.settings}
            onsave={(settings: any) => {}}
        />
        <GameSettingsModal
            bind:isOpen={isGameSettingsModalOpen}
            canModify={true}
            settings={data.gameSettings}
            onsave={(settings: any) => {}}
        />
    {:else}
        <Heading tag="h4" class="text-center">Lobby nie istnieje</Heading>
        <Button href="/panel/lobby">Powrót</Button>
    {/if}
</main>
