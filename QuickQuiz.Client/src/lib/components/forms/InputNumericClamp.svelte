<script lang="ts">
    import { Input, Label } from "flowbite-svelte";

    let { value = $bindable() as number,
        disabled,
        min,
        max,
        children
     } = $props();
</script>

<Label class="font-bold">{@render children()}</Label>
<Input
    onblur={() => {
        if (value < min) value = min;
        if (value > max) value = max;
    }}
    {disabled}
    type="number"
    {min}
    {max}
    bind:value={value}
/>