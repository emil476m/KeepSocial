export interface newAccount {
  userDisplayName: string
  userEmail: string
  userBirthday: Date
  password: string
  AvatarUrl: string
}

export interface Account {
  userId: number
  userDisplayName: string
  userEmail: string
  userBirthday: Date
  avatarUrl: string
}

export interface Profile {
  userId: number
  userDisplayName: string
  avatarUrl: string
  profileDescription: string
  postAmount: number
  followers: number
  following: number
  isFriend: boolean
  isFollowing: boolean
  isSelf: boolean
}

export interface SimpleUser
{
  userId: number
  userDisplayname: string
  avatarUrl?: string
}


export interface BoolResponse
{
  isTrue: boolean
}
