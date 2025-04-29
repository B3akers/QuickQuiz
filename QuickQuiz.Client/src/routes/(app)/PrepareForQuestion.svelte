<script lang="ts">
    import { Card, Heading } from "flowbite-svelte";
    import { UserName } from "$lib/components";
    import { getContext } from "svelte";
    import { type Writable } from "svelte/store";

    const { gamePlayers, prepareForQuestion }: any = getContext("gameState");
    const session: Writable<any> = getContext("session");
</script>

<svelte:head>
    {#if $prepareForQuestion.preloadImage}
        <link
            rel="preload"
            as="image"
            href={$prepareForQuestion.preloadImage}
        />
    {/if}
</svelte:head>

<div class="flex-grow flex flex-col items-center justify-center space-y-4">
    <Card>
        <div class="grid justify-center text-center">
            <span class="flex justify-center">
                <img
                    class="mb-2"
                    width="150"
                    height="150"
                    alt={$prepareForQuestion.category.label}
                    src={$prepareForQuestion.category.icon}
                />
            </span>
            <Heading tag="h3">
                {$prepareForQuestion.category.label}
            </Heading>
            <Heading tag="h5">
                {#if $prepareForQuestion.questionIndex >= $prepareForQuestion.questionCount}
                    Wynik
                {:else}
                    Pytanie <span class="font-bold"
                        >{$prepareForQuestion.questionIndex +
                            1}/{$prepareForQuestion.questionCount}</span
                    > przygotuj się!
                {/if}
            </Heading>
        </div>
        <div class="overflow-x-scroll mt-5">
            <table class="w-full mb-2">
                <thead class="bg-gray-700 text-center">
                    <tr>
                        {#each { length: $prepareForQuestion.questionCount }, i}
                            <th>{i + 1}</th>
                        {/each}
                    </tr>
                </thead>
                <tbody class="text-white text-center">
                    <tr>
                        {#each { length: $prepareForQuestion.questionCount }, i}
                            {#if typeof $gamePlayers[$session.id].roundAnswers[i] == "boolean"}
                                <td
                                    >{$gamePlayers[$session.id].roundAnswers[i]
                                        ? "✅"
                                        : "❌"}</td
                                >
                            {:else}
                                <td>🔘</td>
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
                        {#each { length: $prepareForQuestion.questionCount }, i}
                            <th>{i + 1}</th>
                        {/each}
                    </tr>
                </thead>
                <tbody class="text-white text-center">
                    {#each Object.values($gamePlayers) as any[] as player}
                        <tr>
                            <td><UserName user={player} /></td>
                            {#each { length: $prepareForQuestion.questionCount }, i}
                                {#if typeof player.roundAnswers[i] == "boolean"}
                                    <td
                                        >{player.roundAnswers[i]
                                            ? "✅"
                                            : "❌"}</td
                                    >
                                {:else}
                                    <td>🔘</td>
                                {/if}
                            {/each}
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>
    </Card>
</div>
