<!-- SearchSelect.svelte -->
<script lang="ts">
    import { Label } from "flowbite-svelte";
    interface KeyValue {
        key: string;
        value: string;
    }

    interface MyProps {
        disabled: boolean;
        items: KeyValue[];
        selected?: string[];
        children?: any;
    }

    let {
        disabled,
        items,
        selected = $bindable<string[]>([]),
        children,
    }: MyProps = $props();

    let descriptor = $derived<Record<string, string>>(
        Object.fromEntries(items.map(({ key, value }) => [key, value])),
    );

    let searchTerm = $state<string>("");
    let isOpen = $state<boolean>(false);

    let filtered = $derived<KeyValue[]>(
        items
            .filter((item) =>
                item.value.toLowerCase().includes(searchTerm.toLowerCase()),
            )
            .filter((item) => !selected.includes(item.key)),
    );

    function choose(key: string) {
        selected.push(key);
    }
    function remove(key: string) {
        selected = selected.filter((i) => i !== key);
    }

    function clickOutside(node: HTMLElement) {
        const handler = (e: MouseEvent) => {
            if (!node.contains(e.target as Node)) isOpen = false;
        };
        document.addEventListener("click", handler, true);
        return {
            destroy() {
                document.removeEventListener("click", handler, true);
            },
        };
    }
</script>

<div class="w-full">
    <Label class="font-bold mb-2">{@render children()}</Label>
    {#if selected.length}
        <div
            role="list"
            aria-label="Selected items"
            class="flex flex-wrap mb-2"
        >
            {#each selected as key (key)}
                <div
                    role="listitem"
                    class="flex items-center rounded-full px-3 py-1 m-1
                   bg-blue-100 text-blue-800 dark:bg-blue-800 dark:text-blue-200"
                >
                    <span>{descriptor[key] ?? "undefined"}</span>
                    <a
                        href={null}
                        role="button"
                        tabindex="0"
                        class="ml-2 text-xl leading-none cursor-pointer"
                        class:pointer-events-none={disabled}
                        aria-label={`Remove ${key}`}
                        onclick={(e) => {
                            e.stopPropagation();
                            remove(key);
                        }}
                    >
                        ×
                    </a>
                </div>
            {/each}
        </div>
    {/if}
    <div use:clickOutside>
        <input
            {disabled}
            type="text"
            bind:value={searchTerm}
            placeholder="Szukaj…"
            aria-haspopup="listbox"
            class="w-full border rounded p-2 focus:outline-none focus:ring
               bg-gray-100 text-gray-900 placeholder-gray-500
               dark:bg-gray-700 dark:text-gray-200 dark:placeholder-gray-400
               dark:border-gray-600"
            onclick={() => (isOpen = true)}
            onfocus={() => (isOpen = true)}
        />

        {#if isOpen}
            <ul
                role="listbox"
                style="width: calc(100% - 2.5rem);"
                class="absolute z-10 mt-1 max-h-40 overflow-auto rounded
                   bg-white text-gray-800 border pr-2 border-gray-300 shadow-lg
                   dark:bg-gray-800 dark:text-gray-200 dark:border-gray-700"
            >
                {#each filtered as item (item)}
                    <li>
                        <a
                            href={null}
                            role="button"
                            tabindex="0"
                            class:pointer-events-none={disabled}
                            class="block cursor-pointer w-full text-left px-4 py-2 hover:bg-gray-100
                       dark:hover:bg-gray-700"
                            onclick={() => choose(item.key)}
                        >
                            {item.value}
                        </a>
                    </li>
                {/each}
                {#if filtered.length === 0}
                    <li
                        class="px-4 py-2 italic text-gray-500 dark:text-gray-400"
                    >
                        Brak wyników
                    </li>
                {/if}
            </ul>
        {/if}
    </div>
</div>
