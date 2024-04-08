export interface Video {
    /**
     * 再生時間
     */
    duration: number;

    /**
     * 現在の再生時間
     */
    currentTime: number;
}

//Videoの実装
export class VideoImpl implements Video {
    
    private _videoElement: HTMLVideoElement;

    constructor(videoElement: HTMLVideoElement) {
        this._videoElement = videoElement;
    }

    get duration() {
        return this._videoElement.duration;
    }

    get currentTime() {
        return this._videoElement.currentTime;
    }
}