<script lang="ts">
    import {
        Modal,
        Toggle,
        Helper,
        Input,
        Label,
        Button,
    } from "flowbite-svelte";

    let {
        isOpen = $bindable() as boolean,
        canModify,
        settings,
        onsave,
    } = $props();

    let twitchMode = $state(settings.twitchMode);
    let maxPlayers = $state(settings.maxPlayers);

    $effect(() => {
        twitchMode = settings.twitchMode;
        maxPlayers = settings.maxPlayers;
    });
</script>

<Modal
    size="xs"
    title="Ustawienia lobby"
    bind:open={isOpen}
    outsideclose
    autoclose
>
    <Toggle disabled={!canModify} bind:checked={twitchMode}>Twitch mode</Toggle>
    <Helper class="select-none">
        Dołączenie do lobby wymaga zalogowania się przez twitch.
    </Helper>

    <Label class="font-bold">Maksymalna liczba osób</Label>
    <Input
        onblur={() => {
            if (maxPlayers < 1) {
                maxPlayers = 1;
            }

            if (maxPlayers > 1000) {
                maxPlayers = 1000;
            }
        }}
        disabled={!canModify}
        type="number"
        min={1}
        max={1000}
        bind:value={maxPlayers}
    />

    <Button
        disabled={!canModify}
        onclick={() => {
            onsave({
                twitchMode,
                maxPlayers,
            });
        }}>Zapisz</Button
    >
</Modal>
