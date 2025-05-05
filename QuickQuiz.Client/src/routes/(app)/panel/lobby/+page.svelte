<script lang="ts">
    import { Card, Button, Heading } from "flowbite-svelte";
    import { UserName } from "$lib/components";
    let { data }: any = $props();
</script>

<main
    class="flex-grow flex flex-col items-center justify-center space-y-4 m-5"
>
    {#if data.activeLobbies.length > 0}
        <div class="max-h-max h-auto overflow-visible flex flex-wrap gap-4">
            {#each data.activeLobbies as lobby}
                <Card size="xs">
                    <Heading class="text-center" tag="h6"
                        >Id: {lobby.id}</Heading
                    >
                    <span class="text-center">
                        <UserName user={lobby.owner} />
                    </span>

                    <hr />
                    <span class="text-center"
                        >Gracze: <b>{lobby.playersCount}</b></span
                    >
                    <Button href="/panel/lobby/details?id={encodeURIComponent(lobby.id)}">Zobacz</Button>
                </Card>
            {/each}
        </div>
    {:else}
        <Heading tag="h3" class="text-center"
            >Aktualnie nie ma żadnych lobby</Heading
        >
        <Button href="/panel">Powrót</Button>
    {/if}
</main>