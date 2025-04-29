<script lang="ts">
    import { Card, Heading, Badge } from "flowbite-svelte";
    import { TimeProgressbar } from "$lib/components";
    import { getContext } from "svelte";
    import type { WebSocketManager } from "$lib/client/websocket";
    import { type Writable } from "svelte/store";

    const websocket: Writable<WebSocketManager> = getContext("websocket");
    const { categoryVote }: any = getContext("gameState");
</script>

<Card size="xl">
    <div class="text-center">
        <div class="space-y-5 p-4 sm:p-6">
            <div class="inline-flex items-center gap-2">
                <Heading tag="h3"
                    >Wybierz kategorie <Badge
                        class="text-lg -translate-y-1"
                        large
                        border
                        >{$categoryVote.categoryIndex}/{$categoryVote.maxCategoryIndex}</Badge
                    ></Heading
                >
            </div>
            <TimeProgressbar
                start={new Date($categoryVote.startTime)}
                end={new Date($categoryVote.endTime)}
            />
        </div>
        <hr class="border-gray-700" />
        <div class="mt-4 mb-4 flex w-full flex-col p-4 sm:p-6">
            <div class="flex flex-wrap gap-1 justify-center items-center">
                {#each $categoryVote.categories as category}
                    <a
                        href={null}
                        onclick={() => {
                            $websocket?.sendMessage({
                                $type: "gameCategoryVote",
                                categoryId: category.id,
                            });
                        }}
                        class="text-center cursor-pointer flex flex-col items-center transform transition-transform duration-200 border-2 p-2 hover:scale-105 {$categoryVote.selectedCategory ==
                        category.id
                            ? 'scale-105 rounded-lg border-blue-500'
                            : 'border-transparent'}"
                    >
                        <img
                            class="mb-2"
                            width="150"
                            height="150"
                            alt={category.label}
                            src={category.icon}
                        />
                        <div
                            class="text-white text-center p-2 rounded-3xl min-w-48"
                            style="background-color: {category.color};"
                        >
                            {category.label}
                        </div>
                    </a>
                {/each}
            </div>
        </div>
    </div>
</Card>
