namespace EmailService.Infrastructure.Postgres.Constants
{
    internal static class SqlQueries
    {
        public static readonly string SqlInsert = @"
                INSERT INTO email_outbox(
                    message_id, 
                    recipient, 
                    subject, 
                    body, 
                    is_html, 
                    created_at, 
                    attempts, 
                    next_attempt_at, 
                    status_id
                )
                VALUES {0}
                ON CONFLICT (message_id) DO NOTHING;
                ";

        public static readonly string SqlFetchAndClaim = @"
                WITH cte AS (
                  SELECT message_id FROM email_outbox
                  WHERE status_id = @StatusPending AND next_attempt_at <= now()
                  ORDER BY next_attempt_at
                  FOR UPDATE SKIP LOCKED
                  LIMIT @BatchSize
                )
                UPDATE email_outbox
                SET status_id = @StatusProgress
                WHERE message_id IN (SELECT message_id FROM cte)
                RETURNING message_id as MessageId, 
                          recipient as Recipient,
                          subject as Subject, 
                          body as Body, 
                          is_html as IsHtml
            ";

        public static readonly string SqlMarkSent = @"UPDATE email_outbox SET status_id = @Status WHERE message_id = @Id;";

        public static readonly string SqlMarkFailed = @"
                UPDATE email_outbox
                SET attempts = attempts + 1,
                    last_error = @Error,
                    next_attempt_at = now() + make_interval(secs => @NextSec),
                    status_id = CASE WHEN attempts >= @MaxAttempts THEN @FailedStatus ELSE @PendingStatus END
                WHERE message_id = @Id;
            ";
    }
}
