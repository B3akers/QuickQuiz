<script lang="ts">
    import { Tooltip } from "flowbite-svelte";
    import { TwitchSolid } from "$lib/icons";

    type UserNameProps = {
        name?: string;
        twitch?: boolean;
    };

    let { name, twitch }: UserNameProps = $props();
</script>

<div class="flex justify-center items-center">
    <b>{name}</b>
    {#if twitch}
        <TwitchSolid color="purple" class="ml-1" size="sm" /><Tooltip
            >Zweryfikowany</Tooltip
        >
    {/if}
</div>
