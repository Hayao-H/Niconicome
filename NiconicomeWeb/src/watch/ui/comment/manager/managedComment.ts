export interface ManagedComment {
  body: string;
  userID: string;
  vposMS: number;
  mail: string;
  postedAt: Date;
  number: number;
  isAdded: boolean;
  type: "naka" | "shita" | "ue";
  color: string;
  innnerIndex: number;
}
