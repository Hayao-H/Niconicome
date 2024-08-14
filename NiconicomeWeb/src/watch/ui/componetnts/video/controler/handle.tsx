import React from "https://esm.sh/react@18.2.0";

export const Handle = ({ percentage }: { percentage: number }) => {
  return (
    <div
      className="sliderHandle"
      style={{ transform: `translateX(${percentage * 100}%)` }}
    >
      <div className="sliderHandleInner">
      </div>
    </div>
  );
};
