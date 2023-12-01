export interface CommentModel {
    id: number
    postId: number
    authorId?: number
    text: string
    imgUrl: string
    created: string
    authorName: string
}
