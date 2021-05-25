SELECT PaymentMethodId, COUNT(PaymentMethodId) as Count FROM trade2Statistics 
GROUP By PaymentMethodId