<script lang="ts">
    import { twMerge } from "tailwind-merge";
    import type { OutlineProps } from "./types";

    let { color = "white", size = "md", title, desc, class: className }: OutlineProps = $props();

    const sizes = {
        xs: "w-3 h-3",
        sm: "w-4 h-4",
        md: "w-5 h-5",
        lg: "w-6 h-6",
        xl: "w-8 h-8",
    };
</script>

<svg
    xmlns="http://www.w3.org/2000/svg"
    class={twMerge("shrink-0", sizes[size], className)}
    {color}
    viewBox="0 0 24 24"
>
    {#if title?.id && title.title}
        <title id={title.id}>{title.title}</title>
    {/if}
    {#if desc?.id && desc.desc}
        <desc id={desc.id}>{desc.desc}</desc>
    {/if}
    <path
        d="M2.149 0l-1.612 4.119v16.836h5.731v3.045h3.224l3.045-3.045h4.657l6.269-6.269v-14.686h-21.314zm19.164 13.612l-3.582 3.582h-5.731l-3.045 3.045v-3.045h-4.836v-15.045h17.194v11.463zm-3.582-7.343v6.262h-2.149v-6.262h2.149zm-5.731 0v6.262h-2.149v-6.262h2.149z"
        fill="currentColor"
        stroke-linecap="round"
        stroke-linejoin="round"
    /></svg
>
