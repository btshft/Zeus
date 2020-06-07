{{ for alert in Alerts }}
{{ if alert.Status == "Firing"  }}
*🔥 {{ alert.Status }} 🔥*
{{ else }}
*{{ alert.Status }}*
{{ end }}
*{{ alert.Labels.alertname }}*
{{ alert.Annotations.message }}
*Начало:* {{ date.to_string alert.StartsAt `%F %H:%M:%S` }}
{{ if alert.Status == "Resolved"  }}
*Завершено:* {{ date.to_string alert.EndsAt `%F %H:%M:%S` }}
{{ end }}
{{ end }}