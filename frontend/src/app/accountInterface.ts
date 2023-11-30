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
