export interface Thumbinfo {

    Large: string;
    Normal: string;
    GetSpecifiedThumbnail(size: ThumbSize): string;
}