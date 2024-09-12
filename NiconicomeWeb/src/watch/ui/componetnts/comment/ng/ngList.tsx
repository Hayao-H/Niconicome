import React, {
  useContext,
  useEffect,
  useState,
} from "https://esm.sh/react@18.2.0";
import { VideoStateContext } from "../../../state/videoState.ts";
import { NGItem, NGItemData } from "./ngItem.tsx";
import { NGType } from "../../../comment/ng/ngHandler.ts";
import "../../../../../shared/Extension/arrayExtension.ts";
import { NGInput } from "./ngInput.tsx";

export const NGList = (
  { isExpanded, close }: { isExpanded: boolean; close: () => void },
) => {
  const { state, dispatch } = useContext(VideoStateContext);
  const [ng, setNG] = useState<NGItemData[]>([]);
  const [isInitialized, setIsInitialized] = useState(false);

  useEffect(() => {
    const getNGs = async () => {
      const rawData = await state.ngHandler!.getNGData();
      const formated: NGItemData[] = [];

      function formatNG(
        source: string[],
        type: NGType,
      ): NGItemData[] {
        const formated = source.map((x) => ({
          ngType: type,
          value: x,
          remove: async () => {
            await state.ngHandler!.removeNG(type, x);
            setIsInitialized(false);
          },
        }));

        return formated;
      }

      formated.pushRange(formatNG(rawData.words, "word"));
      formated.pushRange(formatNG(rawData.users, "user"));
      formated.pushRange(formatNG(rawData.commands, "command"));

      setIsInitialized(true);
      setNG(formated);
    };

    getNGs();
  }, [isInitialized]);

  return (
    <div className={`ngWrapper ${isExpanded ? "" : "collapsed"}`}>
      <div className="listControler">
        <p>NG設定</p>
        <p className="close" onClick={close}>
          <i className="fa-solid fa-x"></i>
        </p>
      </div>

      <div className="ngList">
        {ng.map((x) => <NGItem key={`${x.value}-${x.ngType}`} data={x} />)}
      </div>
      <NGInput onAdd={() => setIsInitialized(false)} />
    </div>
  );
};
