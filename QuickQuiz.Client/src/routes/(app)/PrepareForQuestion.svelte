<script lang="ts">
    import { Card, Heading } from "flowbite-svelte";
    import { UserName } from "$lib/components";
    import { getContext } from "svelte";
    import { type Writable } from "svelte/store";

    const game: Writable<any> = getContext("gameState");
    const session: Writable<any> = getContext("session");
</script>

<svelte:head>
    {#if $game.prepareForQuestion.preloadImage}
        <link
            rel="preload"
            as="image"
            href={$game.prepareForQuestion.preloadImage}
        />
    {/if}
</svelte:head>

<div class="flex-grow flex flex-col items-center justify-center space-y-4">
    <Card>
        <div class="grid justify-center text-center">
            <span class="flex justify-center">
                <img
                    width="150"
                    height="150"
                    alt={$game.prepareForQuestion.category.label}
                    src={$game.prepareForQuestion.category.icon}
                />
            </span>
            <Heading tag="h3">
                {$game.prepareForQuestion.category.label}
            </Heading>
            <Heading tag="h5">
                {#if $game.prepareForQuestion.questionIndex >= $game.prepareForQuestion.questionCount}
                    Wynik
                {:else}
                    Pytanie <span class="font-bold"
                        >{$game.prepareForQuestion.questionIndex + 1}/{$game
                            .prepareForQuestion.questionCount}</span
                    > przygotuj się!
                {/if}
            </Heading>
        </div>
    </Card>
    <Card>
        <div class="overflow-x-scroll">
            <table class="w-full mb-2">
                <thead class="bg-gray-700 text-center">
                    <tr>
                        {#each { length: $game.prepareForQuestion.questionCount }, i}
                            <th>{i + 1}</th>
                        {/each}
                    </tr>
                </thead>
                <tbody class="text-white text-center">
                    <tr>
                        {#each { length: $game.prepareForQuestion.questionCount }, i}
                            {#if typeof $game.gamePlayers[$session.id].roundAnswers[i] == "boolean"}
                                <td
                                    >{$game.gamePlayers[$session.id]
                                        .roundAnswers[i]
                                        ? "✅"
                                        : "❌"}</td
                                >
                            {:else}
                                <td>●</td>
                            {/if}
                        {/each}
                    </tr>
                </tbody>
            </table>
        </div>
    </Card>
    <Card>
        <div class="overflow-scroll max-h-80">
            <table class="w-full mb-2">
                <thead class="bg-gray-700">
                    <tr class="text-center">
                        <th>Użytkownik</th>
                        {#each { length: $game.prepareForQuestion.questionCount }, i}
                            <th>{i + 1}</th>
                        {/each}
                    </tr>
                </thead>
                <tbody class="text-white text-center">
                    {#each Object.values($game.gamePlayers) as any[] as player}
                        <tr>
                            <td><UserName user={player} /></td>
                            {#each { length: $game.prepareForQuestion.questionCount }, i}
                                {#if typeof player.roundAnswers[i] == "boolean"}
                                    <td
                                        >{player.roundAnswers[i]
                                            ? "✅"
                                            : "❌"}</td
                                    >
                                {:else}
                                    <td>●</td>
                                {/if}
                            {/each}
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>
    </Card>
</div>
