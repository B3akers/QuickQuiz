export type TitleType = {
    id?: string;
    title?: string;
};

export type DescType = {
    id?: string;
    desc?: string;
};

export interface OutlineProps {
    title?: TitleType;
    desc?: DescType;
    color?: string | undefined | null;
    class?: string | undefined | null;
    size?: 'xs' | 'sm' | 'md' | 'lg' | 'xl';
}