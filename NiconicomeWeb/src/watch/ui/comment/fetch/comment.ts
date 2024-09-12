export interface Comments {
  comments: CommentType[];
}

export interface CommentType {
  body: string;
  userID: string;
  vposMS: number;
  mail: string;
  postedAt: Date;
  number: number;
}
