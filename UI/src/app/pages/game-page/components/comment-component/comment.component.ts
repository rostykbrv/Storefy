import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { forkJoin } from 'rxjs';
import { DatePipe } from '@angular/common';
import { BaseComponent } from 'src/app/componetns/base.component';
import { Comment } from 'src/app/models/comment.model';
import { UserService } from 'src/app/services/user.service';
import { CommentAction } from './comment-action';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'gamestore-comment',
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.scss'],
})
export class CommentComponent extends BaseComponent implements OnInit {
  @Input()
  comment!: Comment;
  formattedDate!: string | null;
  public form: FormGroup;

  @Output()
  select = new EventEmitter<{ comment: Comment; action: CommentAction }>();

  @Output()
  deleteComment = new EventEmitter<Comment>();

  @Output()
  banComment = new EventEmitter<Comment>();

  action = CommentAction;

  canReply = false;
  canQuote = false;
  canDelete = false;
  canBan = false;

  constructor(
    private datePipe: DatePipe,
    private fb: FormBuilder,
    private userService: UserService
    ) {
    super();
    this.form = this.fb.group({
      rating3: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    const commentDate = new Date(this.comment.commentAdded);
    this.formattedDate = this.datePipe.transform(commentDate, 'dd/MM/yyyy')
    forkJoin({
      canReply: this.userService.checkAccess('ReplyComment', this.comment.id),
      canQuote: this.userService.checkAccess('QuoteComment', this.comment.id),
      canDelete: this.userService.checkAccess('DeleteComment', this.comment.id),
      canBan: this.userService.checkAccess('BanComment', this.comment.id),
    }).subscribe((x) => {
      this.canReply = x.canReply;
      this.canQuote = x.canQuote;
      this.canDelete = x.canDelete;
      this.canBan = x.canBan;
    });
  }

  onAction(action: CommentAction): void {
    this.select.emit({ comment: this.comment, action });
  }

  onLike() {
    const currentRating = this.form.get('rating3')?.value || 0;
    const newRating = currentRating < 5 ? currentRating + 1 : 5;
    this.form.patchValue({
      rating3: newRating
    });  
  }
}
