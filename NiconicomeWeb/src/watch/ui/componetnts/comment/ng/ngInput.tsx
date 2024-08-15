import React, {
  useContext,
  useRef,
  useState,
} from "https://esm.sh/react@18.2.0";
import { VideoStateContext } from "../../../state/videoState.ts";
import { NGType } from "../../../comment/ng/ngHandler.ts";

export const NGInput = ({ onAdd }: { onAdd: () => void }) => {
  const { state, dispatch } = useContext(VideoStateContext);
  const [selectValue, setSelectValue] = useState<NGType>("word");
  const [inputValue, setInputValue] = useState("");

  async function addNG() {
    if (inputValue === "") return;

    await state.ngHandler!.addNG(selectValue, inputValue);
    setInputValue("");
    onAdd();
  }

  return (
    <div className="ngInput">
      <select
        className="form-select ngSelect"
        value={selectValue}
        onChange={(e) => setSelectValue(e.target.value as NGType)}
      >
        <option selected>NGの種類</option>
        <option value="word">コメント</option>
        <option value="command">コマンド</option>
        <option value="user">ユーザーID</option>
      </select>
      <div className="input-group mb-3">
        <input
          type="text"
          className="form-control"
          placeholder={`${
            selectValue === "user"
              ? "ユーザーID"
              : selectValue === "command"
              ? "コマンド"
              : "コメント"
          }を入力`}
          value={inputValue}
          onChange={(e) => setInputValue(e.target.value)}
        />

        <button
          className="btn btn-outline-secondary"
          type="button"
          onClick={addNG}
        >
          追加
        </button>
      </div>
    </div>
  );
};
