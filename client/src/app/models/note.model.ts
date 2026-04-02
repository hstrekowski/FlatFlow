export interface NoteDto {
  id: string;
  title: string;
  content: string;
  authorId: string;
}

export interface AddNoteRequest {
  flatId: string;
  title: string;
  content: string;
  authorId: string;
}

export interface UpdateNoteRequest {
  noteId: string;
  title: string;
  content: string;
}
