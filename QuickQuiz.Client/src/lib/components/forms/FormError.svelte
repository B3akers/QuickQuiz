<script lang="ts">
    import { Helper } from "flowbite-svelte";
    import { ExclamationCircleOutline } from "flowbite-svelte-icons";

    let { errors } = $props();
</script>

{#if errors && errors.length != 0}
    {#each errors as error}
        <Helper class="mt-2 flex gap-1 items-center align-middle" color="red"
            ><ExclamationCircleOutline />
            {error}
        </Helper>
    {/each}
{/if}
