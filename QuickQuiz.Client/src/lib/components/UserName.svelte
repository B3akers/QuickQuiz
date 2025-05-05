<script lang="ts">
    import { Tooltip } from "flowbite-svelte";
    import { TwitchSolid, DeveloperSolid } from "$lib/icons";

    let { user }: any = $props();
</script>

<div class="inline-flex items-center">
    <b>{user.name}</b>
    {#if user.id == "twitch-491518421"}
        <DeveloperSolid color="green" class="ml-1" size="md" /><Tooltip
            >Developer</Tooltip
        >
    {/if}
    {#if user.authSource == 'twitch'}
        <TwitchSolid color="purple" class="ml-1" size="sm" /><Tooltip
            >Zweryfikowany</Tooltip
        >
    {/if}
</div>
