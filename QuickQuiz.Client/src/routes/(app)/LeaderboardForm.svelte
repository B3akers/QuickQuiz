<script lang="ts">
    import {
        Card,
        Button,
        Heading,
        Table,
        TableBody,
        TableBodyCell,
        TableBodyRow,
        TableHead,
        TableHeadCell,
    } from "flowbite-svelte";
    import { getContext } from "svelte";
    import { UserName } from "$lib/components";
    import type { WebSocketManager } from "$lib/client/websocket";
    import { type Writable } from "svelte/store";

    const { gamePlayers }: any = getContext("gameState");
    const session: any = getContext("session");
    const websocket: Writable<WebSocketManager> = getContext("websocket");

    const sortedPlayerList = $derived.by(() => {
        const list = Object.values($gamePlayers) as any[];
        list.sort((a: any, b: any) => b.points - a.points);
        return list;
    });
</script>

<main
    class="flex-grow flex flex-col items-center justify-center space-y-4 mt-5 mb-5"
>
    <Card size="md" class="space-y-5">
        <Heading class="text-center" tag="h3">Wyniki końcowe</Heading>
        <Table>
            <TableHead>
                <TableHeadCell>Nr</TableHeadCell>
                <TableHeadCell>Użytkownik</TableHeadCell>
                <TableHeadCell>Punkty</TableHeadCell>
            </TableHead>
            <TableBody tableBodyClass="divide-y">
                {#each sortedPlayerList as player, index}
                    <TableBodyRow>
                        <TableBodyCell
                            class={player.id == $session.id
                                ? "bg-gray-900"
                                : ""}>{index + 1}</TableBodyCell
                        >
                        <TableBodyCell
                            class={player.id == $session.id
                                ? "bg-gray-900"
                                : ""}><UserName user={player} /></TableBodyCell
                        >
                        <TableBodyCell
                            class={player.id == $session.id
                                ? "bg-gray-900"
                                : ""}>{player.points.toFixed(2)}</TableBodyCell
                        >
                    </TableBodyRow>
                {/each}
            </TableBody>
        </Table>
        <Button
            onclick={() => {
                $websocket?.sendMessage({
                    $type: "gameState",
                });
            }}>Powrót do lobby</Button
        >
    </Card>
</main>
