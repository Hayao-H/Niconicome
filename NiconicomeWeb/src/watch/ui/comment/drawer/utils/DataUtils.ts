interface processor {
    splitter: (value: string, splitBy: string) => Array<string>
}


export class DataUtils implements processor {
    splitter(value: string, splitBy: string = ',') {
        if (value) {
            return value.split(splitBy)
        } else {
            return []
        };
    }
    //0埋め
    padding(num: number, digit: number = 2, paddString: string = '0'): string {
        if (num < 10 ** digit) {
            const origin: string = num.toString();
            let padded: string = origin;
            for (let i: number = 0; i < digit - origin.length; i++) {
                padded = paddString + padded;
            }
            return padded;
        } else {
            return num.toString();
        }
    }

    /**
     * ネストされた配列を返します。
     * @param len 配列長
     */
    createNestedArray<K>(len:number):Array<Array<K>>{
        const arr:Array<any>=[]
        for (let i=0;i<len;i++){
            arr.push([]);
        }
        return arr;
    }

    createArray<K>(len:number):Array<K>{
        const arr:Array<K>=[];
        arr.length=len;
        return arr;
    }

    /**
     * VPOSから時間を計算します
     * @param vpos VPOS
     */
    calcVPOS(vpos:number):{ms:{min:number,sec:number},sec:number}{
        const second:number=Math.floor(vpos/100);
        const minute:number=Math.floor(second/60);
        const extra:number=Math.floor(second%60);

        return {ms:{min:minute,sec:extra},sec:second}
    }
}

