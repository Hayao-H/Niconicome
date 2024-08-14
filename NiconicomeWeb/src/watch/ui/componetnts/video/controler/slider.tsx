import React from "https://esm.sh/react@18.2.0";

export const Slider = (
  { percentage, className }: { percentage: number; className: string },
) => {
  return (
    <div className="slider">
      <div
        className={`sliderInner ${className}`}
        style={{ transform: `scaleX(${percentage})` }}
      >
      </div>
    </div>
  );
};
