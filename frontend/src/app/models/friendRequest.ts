export interface FriendRequestModel {
  requestId: number;
  requestersId: number;
  requesterName: string;
  requesterAvatarurl: string | null;
}

export interface RequestUpdateDto {
  requesterId: number;
  requestId: number;
  response: boolean;
}
