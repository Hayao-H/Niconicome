import React from "https://esm.sh/react@18.2.0";
import { NGType } from "../../../comment/ng/ngHandler.ts";

export const NGItem = (
  { data }: { data: NGItemData },
) => {
  let typeStr = "";
  switch (data.ngType) {
    case "word":
      typeStr = "コメント";
      break;
    case "user":
      typeStr = "ユーザー";
      break;
    case "command":
      typeStr = "コマンド";
  }

  return (
    <div className="ngItem">
      <div className="content">
        <div className="type">{typeStr}</div>
        <div className="value">{data.value}</div>
      </div>
      <div className="remove" onClick={data.remove}>
        <i className="fa-solid fa-x"></i>
      </div>
    </div>
  );
};

export interface NGItemData {
  /**
   * 値
   */
  value: string;

  /**
   * NGの種類
   */
  ngType: NGType;

  /**
   * NGを削除
   * @returns
   */
  remove: () => void;
}
