import { DotNetObjectReference } from '../../shared/DotNetObjectReference';
import { ElementHandler } from '../../shared/ElementHandler';

export interface DropHandler {
    Initialize(): void;
}

export class DropHandlerImpl implements DropHandler {
    constructor(dotnet: DotNetObjectReference) {
        this._dotnet = dotnet;
    }

    //#region  field

    private readonly _dotnet: DotNetObjectReference;

    //#endregion
    public Initialize(): void {

        window.addEventListener('dragover', e => e.preventDefault());
        window.addEventListener('drop', e => {
            e.preventDefault();

            if (e.dataTransfer === null) {
                return;
            }

            const targetList: string[] = [];

            e.dataTransfer.types.forEach(t => {
                if (t === 'text/plain') {
                    const data = e.dataTransfer!.getData('text/plain');
                    if (data === '') return;
                    targetList.push(data);
                }
            });

            if (e.dataTransfer.types.includes('Files')) {
                for (let i = 0; i < e.dataTransfer.files.length; i++) {
                    const file = e.dataTransfer.files.item(i);
                    if (file === null) continue;
                    if (file.name === '') continue;
                    if (file.name.endsWith('.url')) continue;
                    targetList.push(file.name);
                }
            }

            const conevrted = targetList.map(t => t.match(/(sm|so|nm)?[0-9]+/)?.[0] ?? '').filter(t => t !== '');
            const distinct = [...(new Set(conevrted))];

            if (distinct.length === 0) {
                return;
            }
            
            const result = distinct.join(' ')

            this._dotnet.invokeMethodAsync('OnDrop', result);
        });
    }
}