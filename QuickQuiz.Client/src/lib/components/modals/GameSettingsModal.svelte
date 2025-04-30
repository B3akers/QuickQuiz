<script module lang="ts">
    import { get } from "$lib/client/requests";

    let _cache: any = null;
    export function loadSettings() {
        if (_cache) return Promise.resolve(_cache);
        return get(fetch, "/game/categories-desc")
            .then((json) => (_cache = json))
            .catch((e) => console.error(e));
    }
</script>

<script lang="ts">
    import { getContext, onMount } from "svelte";
    import { Button, Modal, Toggle, Helper } from "flowbite-svelte";
    import { InputNumericClamp, InputSerachSelect } from "$lib/components";
    import type { WebSocketManager } from "$lib/client/websocket";
    import { type Writable } from "svelte/store";

    const session: any = getContext("session");
    const websocket: Writable<WebSocketManager> = getContext("websocket");
    const { lobby, lobbyGameSettings }: any = getContext("gameState");

    let { isOpen = $bindable() as boolean } = $props();

    let localIsOwner = $derived.by(() => {
        return $lobby.ownerId == $session.id;
    });

    let settings: any = $state(undefined);

    onMount(async () => {
        settings = (await loadSettings()).map((x: any) => {
            return { key: x.id, value: x.label };
        });
    });

    let categoryCountInVote = $state(
        $lobbyGameSettings.categoryCountInVote ?? 12,
    );
    let maxCategoryVotesCount = $state(
        $lobbyGameSettings.maxCategoryVotesCount ?? 5,
    );
    let categoryVoteTimeInSeconds = $state(
        $lobbyGameSettings.categoryVoteTimeInSeconds ?? 15,
    );
    let questionCountPerRound = $state(
        $lobbyGameSettings.questionCountPerRound ?? 5,
    );
    let questionAnswerTimeInSeconds = $state(
        $lobbyGameSettings.questionAnswerTimeInSeconds ?? 15,
    );
    let calculatePointsTimeFactor = $state(
        $lobbyGameSettings.calculatePointsTimeFactor ?? true,
    );
    let calculatePointsDifficultyFactor = $state(
        $lobbyGameSettings.calculatePointsDifficultyFactor ?? true,
    );

    let includeCategories = $state(
        ($lobbyGameSettings.includeCategories ?? []) as string[],
    );
    let excludeCategories = $state(
        ($lobbyGameSettings.excludeCategories ?? []) as string[],
    );

    $effect(() => {
        const settings = $lobbyGameSettings;

        categoryCountInVote = settings.categoryCountInVote ?? 12;
        maxCategoryVotesCount = settings.maxCategoryVotesCount ?? 5;
        categoryVoteTimeInSeconds = settings.categoryVoteTimeInSeconds ?? 15;
        questionCountPerRound = settings.questionCountPerRound ?? 5;
        questionAnswerTimeInSeconds =
            settings.questionAnswerTimeInSeconds ?? 15;
        calculatePointsTimeFactor = settings.calculatePointsTimeFactor ?? true;
        calculatePointsDifficultyFactor =
            settings.calculatePointsDifficultyFactor ?? true;

        includeCategories = (settings.includeCategories ?? []) as string[];
        excludeCategories = (settings.excludeCategories ?? []) as string[];
    });

    let activeTab = $state("general");
</script>

<Modal
    size="xs"
    title="Ustawienia gry"
    bind:open={isOpen}
    outsideclose
    autoclose
>
    <nav class="mb-4 bg-gray-700 px-4 py-2 rounded-t-md">
        <ul class="flex space-x-4">
            <li>
                <a
                    href={null}
                    class="inline-block pb-2 border-b-2 cursor-pointer font-medium
                     transition-colors
                     {activeTab === 'general'
                        ? 'border-blue-400 text-white'
                        : 'border-transparent text-gray-400 hover:text-gray-200 hover:border-gray-500'}"
                    onclick={() => (activeTab = "general")}
                >
                    Ogólne
                </a>
            </li>
            <li>
                <a
                    href={null}
                    class="inline-block pb-2 border-b-2 cursor-pointer font-medium
                     transition-colors
                     {activeTab === 'category'
                        ? 'border-blue-400 text-white'
                        : 'border-transparent text-gray-400 hover:text-gray-200 hover:border-gray-500'}"
                    onclick={() => (activeTab = "category")}
                >
                    Kategorie
                </a>
            </li>
            <li>
                <a
                    href={null}
                    class="inline-block pb-2 border-b-2 cursor-pointer font-medium
                     transition-colors
                     {activeTab === 'advanced'
                        ? 'border-blue-400 text-white'
                        : 'border-transparent text-gray-400 hover:text-gray-200 hover:border-gray-500'}"
                    onclick={() => (activeTab = "advanced")}
                >
                    Punkty
                </a>
            </li>
        </ul>
    </nav>

    {#if activeTab === "general"}
        <div class="overflow-y-scroll max-h-[50vh] space-y-2">
            <InputNumericClamp
                disabled={!localIsOwner}
                min={1}
                max={50}
                bind:value={categoryCountInVote}
                >Ilość kategorii przy głosowaniu</InputNumericClamp
            >
            <InputNumericClamp
                disabled={!localIsOwner}
                min={1}
                max={15}
                bind:value={maxCategoryVotesCount}
                >Ilość rund (każda runda to nowa kategoria)</InputNumericClamp
            >
            <InputNumericClamp
                disabled={!localIsOwner}
                min={1}
                max={20}
                bind:value={questionCountPerRound}
                >Ilośc pytań na każdą runde</InputNumericClamp
            >
            <InputNumericClamp
                disabled={!localIsOwner}
                min={2}
                max={90}
                bind:value={categoryVoteTimeInSeconds}
                >Limit czasowy podczas wyboru kategorii</InputNumericClamp
            >
            <InputNumericClamp
                disabled={!localIsOwner}
                min={2}
                max={120}
                bind:value={questionAnswerTimeInSeconds}
                >Limit czasowy podczas odpowiadania na pytania</InputNumericClamp
            >
        </div>
    {:else if activeTab == "category"}
        <InputSerachSelect
            items={settings}
            bind:selected={excludeCategories}
            disabled={!localIsOwner}>Kategorie wykluczone</InputSerachSelect
        >
        <Helper class="select-none">
            Wybrane kategorie nigdy nie pojawią się w grze.
        </Helper>

        <InputSerachSelect
            items={settings}
            bind:selected={includeCategories}
            disabled={!localIsOwner}>Kategorie wybrane</InputSerachSelect
        >
        <Helper class="select-none">
            Wybrane kategorie zawsze bedą sie pojawiać dopóki nie zostaną
            wybrane.
        </Helper>
    {:else if activeTab == "advanced"}
        <Toggle
            disabled={!localIsOwner}
            bind:checked={calculatePointsTimeFactor}>Punkty za czas</Toggle
        >
        <Helper class="select-none">
            Przyznaje dodatkowe punkty jeżeli czas naszej poprawnej odpowiedzi
            był szybszy niż średnia czasu innych graczy.
        </Helper>

        <Toggle
            disabled={!localIsOwner}
            bind:checked={calculatePointsDifficultyFactor}
            >Punkty za trudność</Toggle
        >
        <Helper class="select-none">
            Przyznaje dodatkowe punkty jeżeli znajdziemy się w mniej niż 50%
            graczy którzy odpowiedzieli poprawnie.
        </Helper>
    {/if}
    <hr class="border-gray-700" />
    <Button
        disabled={!localIsOwner}
        onclick={() => {
            $websocket?.sendMessage({
                $type: "lobbyGameUpdateSettings",
                settings: {
                    categoryCountInVote,
                    maxCategoryVotesCount,
                    categoryVoteTimeInSeconds,
                    questionCountPerRound,
                    questionAnswerTimeInSeconds,
                    calculatePointsTimeFactor,
                    calculatePointsDifficultyFactor,
                    includeCategories,
                    excludeCategories,
                },
            });
        }}>Zapisz</Button
    >
</Modal>
