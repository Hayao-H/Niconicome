import React from "https://esm.sh/react@18.2.0";
import { SliderWrapper } from "./sliderWrapper.tsx";
import { PlayControler } from "./playCOntroler.tsx";
import { TimeControler } from "./timeControler.tsx";
import { GeneralControler } from "./generalControler.tsx";

export const Controler = () => {
  return (
    <div className="controler">
        <SliderWrapper />
        <div className="panel">
            <PlayControler />
            <TimeControler />
            <GeneralControler />
        </div>
    </div>
  );
};
