export interface CommentModel {
    id: number
    post_id: number
    author_id?: number
    text: string
    img_url: string
    created: string
    name: string
}
