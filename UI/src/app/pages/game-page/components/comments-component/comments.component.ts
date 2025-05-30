import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { BaseComponent } from 'src/app/componetns/base.component';
import { Comment } from 'src/app/models/comment.model';
import { CommentService } from 'src/app/services/comment.service';
import { CommentAction } from '../comment-component/comment-action';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { DeleteWrapperComponent } from 'src/app/componetns/delete-wrapper-component/delete-wrapper.component';
import { switchMap, takeUntil, tap } from 'rxjs/operators';
import { BanUserComponent } from '../ban-user-component/ban-user.component';
import { UserService } from 'src/app/services/user.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';;

@Component({
  selector: 'gamestore-comments',
  templateUrl: './comments.component.html',
  styleUrls: ['./comments.component.scss'],
})
export class CommentsComponent extends BaseComponent implements OnInit {
  @Input()
  gameKey!: string;

  readonly clearCommentInput = new Subject();

  public form: FormGroup;
  comments: Comment[] = [];

  canAdd = false;
  writeComment = false;

  selectedComment?: { comment: Comment; action: CommentAction };

  constructor(
    private commentsService: CommentService,
    private userService: UserService,
    private dialog: MatDialog,
    private fb: FormBuilder
  ) {
    super();
    this.form = this.fb.group({
      rating2: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.userService
      .checkAccess('AddComment', this.gameKey)
      .subscribe((x) => (this.canAdd = x));
    this.loadComments();
  }

  onSelect(selected: { comment: Comment; action: CommentAction }): void {
    this.onSelectCancel();

    this.selectedComment = selected;
  }

  onSave(comment: Comment): void {
    this.commentsService
      .addComment(
        comment,
        this.gameKey,
        this.selectedComment?.comment?.id ?? '',
        this.selectedComment?.action ?? ''
      )
      .subscribe((x) => {
        this.onSelectCancel();
        this.clearCommentInput.next();
        if (!!x?.length) {
          this.comments = x;
        } else {
          this.loadComments();
        }
      });
  }

  onSelectCancel(): void {
    this.selectedComment = undefined;
  }

  onDeleteComment(comment: Comment): void {
    const deleteDialog = this.dialog.open(DeleteWrapperComponent);
    deleteDialog.componentInstance.name = comment.name;

    this.handleDialog(
      deleteDialog,
      deleteDialog.componentInstance.delete,
      (_) =>
        this.commentsService
          .deleteComment(comment?.id ?? '', this.gameKey)
          .pipe(tap(() => this.loadComments()))
    );
  }

  onBanComment(comment: Comment): void {
    const banDialog = this.dialog.open(BanUserComponent);
    banDialog.componentInstance.name = comment.name;

    this.handleDialog(banDialog, banDialog.componentInstance.ban, (x) =>
      this.commentsService
        .banUser(x, comment.name)
        .pipe(tap(() => this.loadComments()))
    );
  }

  private handleDialog<TComponent, TEvent, TResponse>(
    dialog: MatDialogRef<TComponent, any>,
    event: EventEmitter<TEvent>,
    eventHandler: (content: TEvent) => Observable<TResponse>
  ) {
    const closed = new Subject();
    event
      .pipe(
        takeUntil(closed),
        tap((_) => dialog.close()),
        switchMap((x) => eventHandler(x))
      )
      .subscribe();

    dialog.afterClosed().subscribe((_) => {
      closed.next();
      closed.complete();
    });
  }

  private loadComments(): void {
    this.commentsService
      .getCommentsByGameKey(this.gameKey)
      .subscribe((x) => (this.comments = x));
  }
}
