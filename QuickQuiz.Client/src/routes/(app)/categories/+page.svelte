<script lang="ts">
    import { Button, Dropdown, DropdownItem, Label } from "flowbite-svelte";

    let { data } = $props();
</script>

<main class="flex-grow flex justify-center items-center">
    <div class="mt-4 mb-4 flex w-full max-w-screen-xl flex-col p-4 sm:p-6">
        <div class="space-y-4">
            <div class="flex justify-center">
                <Button href="/" type="button">Powrót do lobby</Button>
            </div>
            <Label class="text-center"
                >Jeżeli chcesz dodać własną kategorie i masz juz przygotowane
                pytania napisz <a href="mailto:support@quickquiz.pl"
                    >support@quickquiz.pl</a
                ></Label
            >
            <hr class="border-gray-700" />
            <div class="flex flex-wrap gap-3 justify-center items-center">
                {#each data.categories as category}
                    <a
                        href={null}
                        class="text-center cursor-pointer flex flex-col items-center transform transition-transform duration-200 hover:scale-105"
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
                    <Dropdown>
                        <DropdownItem>Dodaj pytanie</DropdownItem>
                        <DropdownItem
                            class="pointer-events-none"
                            disabled
                            slot="footer"
                            >Ilość pytań: <i>{category.questionCount}</i
                            ></DropdownItem
                        >
                    </Dropdown>
                {/each}
            </div>
        </div>
    </div>
</main>
