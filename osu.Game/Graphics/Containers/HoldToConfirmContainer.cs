﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Configuration;

namespace osu.Game.Graphics.Containers
{
    public abstract class HoldToConfirmContainer : Container
    {
        public const double DANGEROUS_HOLD_ACTIVATION_DELAY = 500;

        private const int fadeout_delay = 200;

        /// <summary>
        /// Whether the associated action is considered dangerous, warranting a longer hold.
        /// </summary>
        public bool IsDangerousAction { get; }

        /// <summary>
        /// The action to perform when a hold successfully completes.
        /// </summary>
        public Action Action;

        /// <summary>
        /// Whether currently in a fired state (and the confirm <see cref="Action"/> has been sent).
        /// </summary>
        public bool Fired { get; private set; }

        private bool confirming;

        /// <summary>
        /// The current activation delay for this control.
        /// </summary>
        public IBindable<double> HoldActivationDelay => holdActivationDelay;

        /// <summary>
        /// The progress of any ongoing hold operation. 0 means no hold has started; 1 means a hold has been completed.
        /// </summary>
        public IBindable<double> Progress => progress;

        /// <summary>
        /// Whether the overlay should be allowed to return from a fired state.
        /// </summary>
        protected virtual bool AllowMultipleFires => false;

        private readonly Bindable<double> progress = new BindableDouble();

        private readonly Bindable<double> holdActivationDelay = new Bindable<double>();

        [Resolved]
        private OsuConfigManager config { get; set; }

        protected HoldToConfirmContainer(bool isDangerousAction = false)
        {
            IsDangerousAction = isDangerousAction;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            if (IsDangerousAction)
                holdActivationDelay.Value = DANGEROUS_HOLD_ACTIVATION_DELAY;
            else
                config.BindWith(OsuSetting.UIHoldActivationDelay, holdActivationDelay);
        }

        protected void BeginConfirm()
        {
            if (confirming || (!AllowMultipleFires && Fired)) return;

            confirming = true;

            this.TransformBindableTo(progress, 1, holdActivationDelay.Value * (1 - progress.Value), Easing.Out).OnComplete(_ => Confirm());
        }

        protected virtual void Confirm()
        {
            Action?.Invoke();
            Fired = true;
        }

        protected void AbortConfirm()
        {
            if (!AllowMultipleFires && Fired) return;

            confirming = false;
            Fired = false;

            this
                .TransformBindableTo(progress, progress.Value)
                .Delay(200)
                .TransformBindableTo(progress, 0, fadeout_delay, Easing.InSine);
        }
    }
}
